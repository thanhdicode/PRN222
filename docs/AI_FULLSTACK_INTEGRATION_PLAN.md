# PRN222 MangaWorkflow AI Fullstack Integration Plan

## Overview
This document outlines the architecture for integrating an AI subsystem into the PRN222 MangaWorkflow project.

## Components
1. **Python API (`MangaWorkflow.AI`)**
   - FastAPI-based microservice.
   - Provides endpoints for segmentation, colorization, and task suggestion.
   - Operates in mock mode for development, and uses YOLO/SAM/U-Net when models are available.

2. **.NET Backend**
   - Application layer: Interfaces and DTOs for AI integration (`IAiVisionClient`).
   - Infrastructure layer: HTTP and Mock clients for the Python API.
   - Domain layer: Entities for tracking model versions, inference requests, regions, and task suggestions.

3. **.NET Worker**
   - Background processing for pending AI inference requests to avoid blocking the main web threads.

4. **Web UI (Mangaka AI Studio)**
   - View, execute, and review AI suggestions.
   - Human-in-the-loop: AI proposes tasks, Mangaka approves them.
