from pydantic import BaseModel


class FileSummaryResponse(BaseModel):
    file_name: str
    resume: str
