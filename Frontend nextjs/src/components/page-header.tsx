// src\components\page-header.tsx
import { IPageHeaderProps } from '@/types/interfaces/page-header.interface';

export function PageHeader({ 
  title, 
  description, 
  badge 
}: IPageHeaderProps) {
  return (
    <>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-4xl font-bold text-foreground">{title}</h1>
          {description && (
            <p className="mt-3 text-sm text-foreground/80">
              {description}
            </p>
          )}
        </div>
        {badge}
      </div>
    </>
  );
}
