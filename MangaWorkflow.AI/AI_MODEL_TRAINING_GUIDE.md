# MangaWorkflow AI Model Training Guide

## Overview
This guide explains how to train and evaluate the AI Region Segmentation models (e.g., YOLOv8-seg, U-Net) for the MangaWorkflow project.

## Requirements
* Python 3.9+
* CUDA-compatible GPU (Highly Recommended)
* Prepared dataset (See `AI_DATASET_PREPARATION_GUIDE.md`)

## Installation
1. Navigate to the `MangaWorkflow.AI` directory.
2. Create and activate a virtual environment:
   ```bash
   python -m venv .venv
   .venv\Scripts\activate
   ```
3. Install dependencies:
   ```bash
   pip install ultralytics fastapi uvicorn
   ```

## Training YOLO Segmentation
We use `yolov8n-seg.pt` as our base model for fast inference.

1. Navigate to the training directory:
   ```bash
   cd training
   ```
2. Start training:
   ```bash
   python train_yolo_seg.py --epochs 100 --batch 16 --imgsz 640
   ```
3. The training results, including the best weights (`best.pt`), will be saved to `MangaWorkflow.AI/runs/segment/manga_yolo_seg/weights/`.

## Running the API
Once training is complete, copy your `best.pt` model to `MangaWorkflow.AI/models/yolo_seg/best.pt`.
Then run the API server to expose the AI for the .NET Backend:
```bash
cd MangaWorkflow.AI
uvicorn src.api.main:app --reload --port 8001
```
