# MangaWorkflow AI Dataset Preparation Guide

## Overview
This guide provides instructions for downloading and preparing datasets for training the MangaWorkflow AI Region Segmentation models.

## Manga109 Dataset
Manga109 is a dataset of 109 Japanese manga volumes compiled by the Aizawa Yamasaki Laboratory at the University of Tokyo. It contains annotations for frames (panels), text, faces, and bodies.

### 1. Request Access
Manga109 is available for academic research. You must apply for access on the [Manga109 website](http://www.manga109.org/en/download.html).

### 2. Download and Extract
Once approved, download the dataset and extract it to the `MangaWorkflow.AI/datasets/raw/Manga109` directory.
The structure should look like:
```
datasets/raw/Manga109/
├── annotations/
│   ├── AisiteIruyoSensei.xml
│   ├── ...
├── images/
│   ├── AisiteIruyoSensei/
│   │   ├── 000.jpg
│   │   ├── ...
```

### 3. Convert to YOLO Format
Our segmentation model relies on Ultralytics YOLO. You must convert the Manga109 XML annotations to YOLO format.

1. Ensure Python dependencies are installed.
2. Run the preparation script:
   ```bash
   cd MangaWorkflow.AI/training
   python prepare_manga109.py
   ```
3. The script will output the normalized YOLO dataset in `datasets/processed/Manga109_Yolo/`.

You are now ready to begin model training!
