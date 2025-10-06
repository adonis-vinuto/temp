from langgraph.graph import MessagesState

class MessageWithPrefsState(MessagesState):
    preferences: dict[str, str]