import { getServerSession } from "next-auth";
import { authOptions } from "../auth";
import { KeycloakToken } from "@/types/interfaces/keycloak-token.interface";
import { jwtDecode } from "jwt-decode";

export async function decodeAccessToken() {
    const session = await getServerSession(authOptions);
    const decoded = jwtDecode<KeycloakToken>(session?.accessToken || '');
    return decoded;
}
