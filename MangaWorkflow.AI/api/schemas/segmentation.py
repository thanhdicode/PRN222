from pydantic import BaseModel
from typing import List, Optional

class SegmentationRequest(BaseModel):
    pageId: str
    imagePath: Optional[str] = None
    imageUrl: Optional[str] = None
    mode: str = "mock" # yolo | yolo_sam | mock

class Point2D(BaseModel):
    x: float
    y: float

class DetectedRegion(BaseModel):
    label: str
    confidence: float
    x: float
    y: float
    width: float
    height: float
    polygon: Optional[List[List[float]]] = None

class SegmentationResponse(BaseModel):
    pageId: str
    modelName: str
    modelVersion: str
    imageUrl: Optional[str] = None  # Echo back the image URL for verification
    detections: List[DetectedRegion]
