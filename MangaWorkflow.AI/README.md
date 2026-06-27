# MangaWorkflow AI Studio

This directory contains the Python-based AI subsystem for MangaWorkflow.

## Setup
1. Create a virtual environment: `python -m venv .venv`
2. Activate it: `.venv\Scripts\activate` (Windows) or `source .venv/bin/activate` (Mac/Linux)
3. Install dependencies: `pip install -r requirements-api.txt`

## Running API
```bash
uvicorn api.main:app --host 0.0.0.0 --port 8001
```
