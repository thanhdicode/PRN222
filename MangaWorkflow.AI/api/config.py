import os
from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    host: str = "0.0.0.0"
    port: int = 8001
    debug: bool = True
    
    yolo_model_path: str = os.getenv("YOLO_MODEL_PATH", "models/yolo/best_manga_seg.pt")
    sam_model_path: str = os.getenv("SAM_MODEL_PATH", "models/sam/sam_vit_h_4b8939.pth")
    unet_model_path: str = os.getenv("UNET_MODEL_PATH", "models/unet/unet_manga.pth")
    colorization_model_path: str = os.getenv("COLORIZATION_MODEL_PATH", "models/colorization/color_model.pth")
    
    confidence_threshold: float = 0.60
    
    class Config:
        env_file = ".env"

settings = Settings()
