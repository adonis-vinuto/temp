// src\types\interfaces\page-header.intf.ts
import { ReactNode } from 'react';

export interface IPageHeaderProps {
  title: string;
  description?: string;
  badge?: ReactNode;
  separator?: boolean;
}
