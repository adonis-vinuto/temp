// src\components\form-field.tsx
import { Input } from '@/components/ui/input';
import { IFormFieldProps } from '@/types/interfaces/form-field.interface';

export function FormField({ 
  label, 
  placeholder, 
  type = 'text',
  required = false,
  optional = false,
  register, 
  error 
}: IFormFieldProps) {
  return (
    <div className="space-y-2">
      <label className="text-sm font-medium text-foreground/80">
        {label} {required && <span className='text-red-600'>*</span>}
        {optional && (
          <span className="text-xs text-foreground/60 ml-1">
            (opcional)
          </span>
        )}
      </label>
      <Input
        {...register}
        type={type}
        placeholder={placeholder}
        className='text-foreground/70 placeholder:text-foreground/40'
      />
      {error && (
        <p className="text-xs text-red-500">{error}</p>
      )}
    </div>
  );
}
