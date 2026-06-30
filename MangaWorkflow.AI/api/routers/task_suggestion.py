from fastapi import APIRouter
from api.schemas.task_suggestion import TaskSuggestionRequest, TaskSuggestionResponse
from api.services import task_suggester

router = APIRouter()

@router.post("/suggest-tasks", response_model=TaskSuggestionResponse)
def suggest_tasks(request: TaskSuggestionRequest):
    return task_suggester.generate_suggestions(request)
