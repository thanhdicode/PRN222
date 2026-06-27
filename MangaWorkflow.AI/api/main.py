from fastapi import FastAPI
from api.routers import health, segmentation, colorization, task_suggestion

app = FastAPI(title="MangaWorkflow AI API", version="1.0.0")

app.include_router(health.router, tags=["Health"])
app.include_router(segmentation.router, prefix="/api", tags=["Segmentation"])
app.include_router(colorization.router, prefix="/api", tags=["Colorization"])
app.include_router(task_suggestion.router, prefix="/api", tags=["Task Suggestion"])

@app.get("/")
def read_root():
    return {"message": "Welcome to MangaWorkflow AI API"}
