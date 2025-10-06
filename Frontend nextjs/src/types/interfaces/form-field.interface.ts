// src\types\interfaces\form-field.intf.ts
import { UseFormRegisterReturn } from 'react-hook-form';

export interface IFormFieldProps {
  label: string;
  placeholder?: string;
  type?: string;
  required?: boolean;
  optional?: boolean;
  register: UseFormRegisterReturn;
  error?: string;
}
