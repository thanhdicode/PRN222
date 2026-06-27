import os
from api.schemas.segmentation import SegmentationRequest, SegmentationResponse, DetectedRegion
from api.config import settings

def segment_page(request: SegmentationRequest) -> SegmentationResponse:
    if request.mode == "mock" or not os.path.exists(settings.yolo_model_path):
        return _mock_segmentation(request)
    
    # Real YOLO inference logic goes here
    # For now, fallback to mock if model is not successfully loaded
    return _mock_segmentation(request)

def _mock_segmentation(request: SegmentationRequest) -> SegmentationResponse:
    detections = [
        DetectedRegion(label="Panel", confidence=0.95, x=10, y=10, width=400, height=300),
        DetectedRegion(label="SpeechBubble", confidence=0.88, x=50, y=50, width=100, height=80),
        DetectedRegion(label="Character", confidence=0.92, x=200, y=150, width=150, height=200),
    ]
    
    return SegmentationResponse(
        pageId=request.pageId,
        modelName="mock-yolo-seg",
        modelVersion="v1",
        detections=detections
    )
