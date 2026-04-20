from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import nmap
import json

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
        return {"status": "success", "data": scan_results}

    except nmap.PortScannerError as e:
        raise HTTPException(status_code=500, detail=f"Nmap execution error: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"An unexpected error occurred: {str(e)}")