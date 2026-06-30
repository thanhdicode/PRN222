from fastapi import APIRouter
from api.schemas.segmentation import SegmentationRequest, SegmentationResponse
from api.services import yolo_segmenter

router = APIRouter()

@router.post("/segment", response_model=SegmentationResponse)
def segment_page(request: SegmentationRequest):
    return yolo_segmenter.segment_page(request)
