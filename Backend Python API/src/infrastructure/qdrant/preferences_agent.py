from langchain_core.messages import SystemMessage

def system_prompt_with_prefs(preferences: dict[str, str], prompt: str) -> str:
    prefs_text = "\n".join([f"- {k}: {v}" for k, v in preferences.items()])
    
    content = f"""{prompt}

    User preferences:
    {prefs_text if prefs_text else "No preferences set yet."}
    """
    return SystemMessage(content)