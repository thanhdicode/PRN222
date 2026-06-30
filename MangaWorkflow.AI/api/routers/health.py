from fastapi import APIRouter
import os
from api.config import settings

router = APIRouter()

@router.get("/health")
def health_check():
    # Simple check for mock vs loaded
    yolo_exists = os.path.exists(settings.yolo_model_path)
    model_status = "loaded" if yolo_exists else "mock"
    
    return {
        "status": "ok",
        "modelStatus": model_status,
        "version": "1.0.0"
    }
