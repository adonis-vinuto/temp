from fastapi import HTTPException, Request, Response
from fastapi.responses import JSONResponse
from ..configs import config

token_ged = config.TOKENGED
token_mind = config.TOKENMIND

async def verify_token(request: Request, call_next):
    if request.url.path == "/health":
        return await call_next(request)

    auth_header = request.headers.get("token")
    if not auth_header:
        return JSONResponse(status_code=401, content={"detail": "Unauthorized"})

    if auth_header != token_ged and auth_header != token_mind:
        return JSONResponse(status_code=401, content={"detail": "Unauthorized"})

    response = await call_next(request)
    return response
