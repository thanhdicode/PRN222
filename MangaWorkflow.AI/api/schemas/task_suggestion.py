from pydantic import BaseModel
from typing import List, Optional
from api.schemas.segmentation import DetectedRegion

class TaskSuggestionRequest(BaseModel):
    pageId: str
    regions: List[DetectedRegion]

class TaskSuggestion(BaseModel):
    title: str
    description: str
    taskType: str
    suggestedAssistant: Optional[str] = None
    estimatedComplexity: float
    estimatedDeadlineHours: float
    estimatedEarning: float

class TaskSuggestionResponse(BaseModel):
    suggestions: List[TaskSuggestion]
