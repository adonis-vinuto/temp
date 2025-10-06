// src/lib/providers/config-provider.tsx
'use client';

import { createContext, useContext, useEffect, useState, useMemo } from 'react';
import { usePathname } from 'next/navigation';
import { useQuery } from '@tanstack/react-query';
import { getDataConfig } from '@/lib/api/data-config.client';
import { ConfigRequiredModal } from '@/components/config-required-modal';

interface ConfigContextType {
  hasConfig: boolean;
  isLoading: boolean;
  refetch: () => void;
}

const ConfigContext = createContext<ConfigContextType>({
  hasConfig: false,
  isLoading: true,
  refetch: () => {},
});

export function useConfig() {
  return useContext(ConfigContext);
}

const ROUTE_CONFIG = {
  exempt: [
    '/settings',
    '/login',
    '/',
    '/api',
    '/_next',
  ],
  protected: [
    '/dashboard',
    '/agents',
    '/users',
    '/files',
    '/knowledge',
    '/integrations',
    '/modules',
  ]
} as const;

export function ConfigProvider({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const [showModal, setShowModal] = useState(false);
  const { data, isLoading, refetch } = useQuery({
    queryKey: ['data-config-check'],
    queryFn: () => getDataConfig(),
    staleTime: 0,
    gcTime: 5 * 60 * 1000,
    retry: 1,
  });

  const hasConfig = !!data;
  const exemptPaths = useMemo(() => ROUTE_CONFIG.exempt, []);
  const protectedPaths = useMemo(() => ROUTE_CONFIG.protected, []);

  useEffect(() => {
    if (isLoading || hasConfig) {
      setShowModal(false);
      return;
    }

    const isExemptPath = exemptPaths.some(path => {
      if (path === '/') {
        return pathname === '/';
      }
      return pathname.startsWith(path);
    });

    if (isExemptPath) {
      setShowModal(false);
      return;
    }

    const isProtectedPath = protectedPaths.some(path =>
      pathname.startsWith(path)
    );

    setShowModal(isProtectedPath);
  }, [pathname, hasConfig, isLoading, exemptPaths, protectedPaths]);

  const contextValue = useMemo(
    () => ({ hasConfig, isLoading, refetch }),
    [hasConfig, isLoading, refetch]
  );

  return (
    <ConfigContext.Provider value={contextValue}>
      {children}
      {showModal && <ConfigRequiredModal />}
    </ConfigContext.Provider>
  );
}
