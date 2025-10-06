export interface KeycloakToken {
  exp: number;
  iat: number;
  auth_time: number;
  jti: string;
  iss: string;
  aud: string;
  sub: string;
  typ: string;
  azp: string;
  sid: string;
  acr: string;
  "allowed-origins": string[];
  realm_access: {
    roles: string[];
  };
  resource_access: {
    [resource: string]: {
      roles: string[];
    };
  };
  scope: string;
  email_verified: boolean;
  organization?: string[];
  name?: string;
  preferred_username?: string;
  given_name?: string;
  family_name?: string;
  locale?: string;
  email?: string;
}
