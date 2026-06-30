# AI Research Alignment for MangaWorkflow

## Research Question (RQ)
Which architecture best detects and segments manga panels, speech bubbles, and characters?

## Proposed Models
- **YOLO Segmentation**: Primary production model for instance segmentation.
- **SAM (Segment Anything Model)**: Used for mask refinement on top of YOLO boxes.
- **U-Net**: Semantic segmentation baseline for comparison.

## Dataset
- **Manga109**: The primary dataset used for training and evaluation. Contains 109 manga books with detailed annotations.
- **Note on Manga109-v2026**: Future evaluations may incorporate this dataset to resolve speech balloon under-segmentation and overlapping texts.

## Evaluation Metrics
- Intersection over Union (IoU)
- Dice / F1 Score
- Mean Average Precision (mAP50, mAP50-95)
- Latency (Inference time)

## Current Status
- **Integration**: Mock integration complete. The .NET core seamlessly interacts with a simulated Python API.
- **Pipeline**: Training scripts and data conversion pipelines are prepared.
- **Real Training**: True model training and weights generation depend on dataset access and GPU availability.
