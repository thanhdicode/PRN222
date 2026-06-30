from fastapi import APIRouter
from api.schemas.colorization import ColorizationRequest, ColorizationResponse
from api.services import colorizer

router = APIRouter()

@router.post("/colorize", response_model=ColorizationResponse)
def colorize_page(request: ColorizationRequest):
    return colorizer.colorize_page(request)
