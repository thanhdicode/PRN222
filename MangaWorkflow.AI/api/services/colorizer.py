import os
from api.schemas.colorization import ColorizationRequest, ColorizationResponse
from api.config import settings

def colorize_page(request: ColorizationRequest) -> ColorizationResponse:
    if request.mode == "mock" or not os.path.exists(settings.colorization_model_path):
        return ColorizationResponse(
            pageId=request.pageId,
            status="completed",
            previewPath="mock/preview/path.jpg"
        )
        
    return ColorizationResponse(
        pageId=request.pageId,
        status="completed",
        previewPath="mock/preview/path.jpg"
    )
