from api.schemas.task_suggestion import TaskSuggestionRequest, TaskSuggestionResponse, TaskSuggestion

def generate_suggestions(request: TaskSuggestionRequest) -> TaskSuggestionResponse:
    suggestions = []
    
    for region in request.regions:
        if region.label == "Panel":
            suggestions.append(TaskSuggestion(
                title="Clean panel layout",
                description="Refine border and clean gutters for the detected panel.",
                taskType="LayoutCleanup",
                estimatedComplexity=2.0,
                estimatedDeadlineHours=1.5,
                estimatedEarning=15.00
            ))
        elif region.label == "SpeechBubble":
            suggestions.append(TaskSuggestion(
                title="Clean speech bubble",
                description="Remove artifact from speech bubble, prepare for lettering.",
                taskType="Lettering",
                estimatedComplexity=1.0,
                estimatedDeadlineHours=0.5,
                estimatedEarning=5.00
            ))
        elif region.label == "Character" or region.label == "CharacterBody":
            suggestions.append(TaskSuggestion(
                title="Character ink/tone refinement",
                description="Apply screen tone and refine ink lines for character.",
                taskType="CharacterInkTone",
                estimatedComplexity=3.0,
                estimatedDeadlineHours=3.0,
                estimatedEarning=30.00
            ))
        elif region.label == "Background":
            suggestions.append(TaskSuggestion(
                title="Background detail rendering",
                description="Add background details behind character.",
                taskType="BackgroundDrawing",
                estimatedComplexity=4.0,
                estimatedDeadlineHours=5.0,
                estimatedEarning=50.00
            ))
        elif region.label == "Effect":
            suggestions.append(TaskSuggestion(
                title="Add screen tone / visual effects",
                description="Enhance visual effects for impact.",
                taskType="EffectsTone",
                estimatedComplexity=2.5,
                estimatedDeadlineHours=2.0,
                estimatedEarning=20.00
            ))
            
    return TaskSuggestionResponse(suggestions=suggestions)
