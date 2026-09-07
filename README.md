# AutoRecon: AI-Powered Network Vulnerability Scanner

## Project Overview
AutoRecon is a web application that automates network reconnaissance. Originally built as a university capstone project, it uses an ASP.NET Core MVC frontend and a FastAPI Python backend to run live Nmap scans, parse the results, and generate threat mitigation reports using the OpenAI API.

This repository demonstrates full-stack development, API integration, and secure credential management.

## Demonstration

[![AutoRecon Demo](https://img.youtube.com/vi/5HGUIQQHLLs/maxresdefault.jpg)](https://www.youtube.com/watch?v=5HGUIQQHLLs)

**Note:** Real-time scans take approximately 2.5 minutes to process because the AI engine generates custom threat summaries for all 18 discovered vulnerabilities. The video above is sped up for demonstration purposes.

## Key Features
* **Automated Reconnaissance:** Executes live Nmap scans against target IP addresses directly from a web interface.
* **AI Threat Analysis:** Parses raw Nmap XML output and leverages OpenAI to generate remediation steps for identified vulnerabilities.
* **Asynchronous Processing:** Utilizes extended HTTP timeouts and asynchronous threading to handle large, time-intensive API payloads without dropping connections.
* **Database Logging:** Archives all scans, raw terminal outputs, and AI summaries into a local SQLite database for historical review.

## Architecture and Technologies
* **Frontend:** C#, ASP.NET Core MVC, HTML/CSS
* **Backend:** Python, FastAPI, Nmap
* **Database:** SQLite, Entity Framework Core
* **AI Integration:** OpenAI API

## Getting Started
**Prerequisites**
* .NET 8.0 SDK or later
* Python 3.10+
* Nmap installed and added to the system PATH
* A valid OpenAI API Key

**Installation**
1. Clone the repository: `git clone https://github.com/jweaver-git/AutoRecon.git`
2. Navigate to the Python engine directory: `cd PythonEngine`
3. Create a virtual environment and install requirements: `pip install -r requirements.txt`
4. Rename `.env.example` to `.env` and insert your OpenAI API key.
5. Launch the backend: `uvicorn main:app --reload`
6. Open `AutoRecon.sln` in Visual Studio and run the C# frontend.

## Security Disclaimer
This tool was built strictly for educational purposes and authorized auditing. Development and testing were conducted exclusively in a controlled local environment against Metasploitable 2, a deliberately vulnerable virtual machine. Do not use AutoRecon to scan networks or IP addresses without explicit, written permission from the network owner.