import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  experimental: {
    serverActions: {
      allowedOrigins: [
        "localhost:3000",
        "144.33.17.76",
        "144.33.17.76:80",
        "144.33.17.76:3000",
      ],
    },
  },
  eslint: {
    ignoreDuringBuilds: true,
  },
  images: {
    domains: ["statumdisco.blob.core.windows.net"],
    remotePatterns: [
      {
        protocol: "http",
        hostname: "127.0.0.1",
        port: "10000",
        pathname: "/devstoreaccount1/gemelli-api/**",
      },
    ],
  },
};

export default nextConfig;
