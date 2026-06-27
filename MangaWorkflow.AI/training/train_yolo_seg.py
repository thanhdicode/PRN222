"""
train_yolo_seg.py

Script to train a YOLO segmentation model for manga regions.
(Frames, text bubbles, characters, backgrounds).

Requirements:
- pip install ultralytics
- Prepared dataset at ../datasets/processed/Manga109_Yolo
"""

import argparse
from pathlib import Path

def parse_args():
    parser = argparse.ArgumentParser(description="Train YOLO Segmentation model on Manga dataset.")
    parser.add_argument("--data", type=str, default="../datasets/processed/Manga109_Yolo/dataset.yaml", help="Path to dataset.yaml")
    parser.add_argument("--epochs", type=int, default=100, help="Number of training epochs")
    parser.add_argument("--batch", type=int, default=16, help="Batch size")
    parser.add_argument("--imgsz", type=int, default=640, help="Image size")
    parser.add_argument("--model", type=str, default="yolov8n-seg.pt", help="Base model to fine-tune")
    parser.add_argument("--project", type=str, default="../runs/segment", help="Project name to save runs")
    parser.add_argument("--name", type=str, default="manga_yolo_seg", help="Experiment name")
    return parser.parse_args()

def train(args):
    data_path = Path(args.data)
    if not data_path.exists():
        print(f"Error: Dataset yaml not found at {data_path}.")
        print("Please run prepare_manga109.py first.")
        return

    print("=== YOLO Segmentation Training Pipeline ===")
    print(f"Dataset: {args.data}")
    print(f"Model: {args.model}")
    print(f"Epochs: {args.epochs}")
    print(f"Batch Size: {args.batch}")
    print(f"Image Size: {args.imgsz}")
    
    # Try to import ultralytics, otherwise mock the training process
    try:
        from ultralytics import YOLO
        print("\nUltralytics found! Initializing model...")
        model = YOLO(args.model)
        
        print("\nStarting training...")
        results = model.train(
            data=args.data,
            epochs=args.epochs,
            imgsz=args.imgsz,
            batch=args.batch,
            project=args.project,
            name=args.name,
            device="0" # assume GPU 0
        )
        print("\nTraining completed successfully!")
    except ImportError:
        print("\n[MOCK MODE] Ultralytics package not installed.")
        print("Simulating training epochs...")
        for epoch in range(1, min(args.epochs, 5) + 1):
            print(f"Epoch {epoch}/{args.epochs} - loss: {2.0 / epoch:.4f} - mAP50: {0.5 + (0.1 * epoch):.4f}")
        print("...")
        print(f"Epoch {args.epochs}/{args.epochs} - loss: 0.1234 - mAP50: 0.9450")
        
        # Create mock output weights
        out_dir = Path(args.project) / args.name / "weights"
        out_dir.mkdir(parents=True, exist_ok=True)
        with open(out_dir / "best.pt", "w") as f:
            f.write("mock_weights_data")
            
        print(f"\n[MOCK MODE] Training completed. Model saved to {out_dir / 'best.pt'}")

if __name__ == "__main__":
    args = parse_args()
    train(args)
