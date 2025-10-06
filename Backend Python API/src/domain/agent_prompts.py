from typing import Dict

from src.domain.chat import UserInfo

MODULE_KNOWLEDGE: Dict[str, str] = {
    "People": (
        "Department responsible for human resources, handling recruitment, "
        "benefits, training and employee relations."
    ),
    "Sales": (
        "Focuses on prospecting, lead management, negotiating deals and "
        "maintaining customer relationships to drive revenue."
    ),
    "Finance": (
        "Manages budgets, payroll, financial planning and reporting to "
        "ensure the company's fiscal health."
    ),
    "Support": (
        "Provides assistance to customers by troubleshooting issues, "
        "managing tickets and maintaining a knowledge base."
    ),
    "Tax": (
        "Ensures compliance with tax regulations, prepares filings and "
        "advises on tax-efficient strategies."
    ),
}

def get_hydrated_system_prompt(module: str | tuple, user: UserInfo, preferences: Dict[str, str] = None) -> str:
    if isinstance(module, tuple):
        module_name = module[0]
    else:
        module_name = module
    module_info = MODULE_KNOWLEDGE.get(module_name, "")
    prompt = (
        f"Your name is Komvos Mind {module_name} and you are an expert planner for the ({module_name}) department.\n"
        "Be helpful and answer the user's questions based on the context provided AND ALWAYS in Portuguese.\n\n"
        "Departament responsibilities:\n"
        f"{module_info}\n\n"
        f"Basic user information:\n"
        f"Name: {user.name}\n"
        f"Organization: {user.organization}\n"
        f"Email: {user.email}\n\n"
    )
    if preferences:
        prompt += "User preferences:\n"
        for key, value in preferences.items():
            prompt += f"- {key}: {value}\n"
    return prompt