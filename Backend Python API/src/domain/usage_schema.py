from pydantic import BaseModel

class UsageSchema(BaseModel):
    prompt_tokens: int
    completion_tokens: int
    total_tokens: int
    completion_time: float
    prompt_time: float
    queue_time: float
    total_time: float