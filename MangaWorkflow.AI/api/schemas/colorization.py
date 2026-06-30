from pydantic import BaseModel
from typing import Optional

class ColorizationRequest(BaseModel):
    pageId: str
    imagePath: Optional[str] = None
    mode: str = "mock"

class ColorizationResponse(BaseModel):
    pageId: str
    status: str
    previewPath: str
