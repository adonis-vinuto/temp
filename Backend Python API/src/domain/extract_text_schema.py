from pydantic import BaseModel
from typing import List, Union

class PageSchema(BaseModel):
    text: str  
    page_number: Union[int, str]

class FileSchema(BaseModel):
    file_name: str
    file_type: str
    resume: str
    pages: List[PageSchema]