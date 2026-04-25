from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from dotenv import load_dotenv
from openai import OpenAI
import os
import nmap
import json

load_dotenv()

# Initialize OpenAI Client
client = OpenAI()

# Initialize FastAPI app
app = FastAPI(title="AutoRecon API")

# Define the data model we expect to receive from C#
class Target(BaseModel):
    ip_address: str

@app.post("/api/scan")
def run_scan(target: Target):
    try:
        # Initialize the Nmap PortScanner
        nm = nmap.PortScanner()
        
        print(f"[*] Starting scan on {target.ip_address}...")
        
        # We use -sT (TCP Connect) instead of -sS so it doesn't require Admin privileges on Windows.
        # We use --top-ports 100 to keep the prototype scan incredibly fast.
        nm.scan(hosts=target.ip_address, arguments='-sT --top-ports 100')
        
        # Check if the host is up
        if target.ip_address not in nm.all_hosts():
            raise HTTPException(status_code=404, detail="Target host seems to be down or unreachable.")
            
        # Parse the raw data into a clean dictionary
        scan_results = {
            "target": target.ip_address,
            "state": nm[target.ip_address].state(),
            "open_ports": []
        }
        
        # Extract open ports and services
        if 'tcp' in nm[target.ip_address]:
            for port in nm[target.ip_address]['tcp'].keys():
                state = nm[target.ip_address]['tcp'][port]['state']
                if state == 'open':
                    service = nm[target.ip_address]['tcp'][port]['name']
                    scan_results["open_ports"].append({
                        "port": port,
                        "service": service
                    })
                    
        print(f"[+] Scan complete. Found {len(scan_results['open_ports'])} open ports.")
        
        # =============
        # AI LAYER
        # =============
        vulnerabilities = []

        # Ensures the AI is only asked to analyze if open ports are found.
        if len(scan_results["open_ports"]) > 0:
            print("[*] Sending port data to OpenAI for vulnerability analysis...")

            # AI Prompt to ensure frontend stability
            system_prompt = """
            You are a Senior Penetration Tester analyzing Nmap scan results.
            Identify potential security vulnerabilities based on the open ports and services provided.
            You MUST return the results STRICTLY as a JSON array of objects.
            Each object must have: 'port' (int), 'service' (string), 'title' (string), severity' (High/Medium/Low), and 'description' (string).
            Do not include any markdown formatting, backticks, or conversational text.
            """

            user_prompt = f"Analyze this scan data: {json.dumps(scan_results['open_ports'])}"

            response = client.chat.completions.create(
                model="gpt-5.4-mini",
                messages=[
                    {"role": "system", "content": system_prompt},
                    {"role": "user", "content": user_prompt}
                ]
            )
            
            raw_ai_text = response.choices[0].message.content.strip()
            vulnerabilities = json.loads(raw_ai_text)
            print("[+] AI analysis complete.")
        
        return {
            "status": "success", 
            "data": scan_results, 
            "vulnerabilities": vulnerabilities
        }

    except nmap.PortScannerError as e:
        raise HTTPException(status_code=500, detail=f"Nmap execution error: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"An unexpected error occurred: {str(e)}")