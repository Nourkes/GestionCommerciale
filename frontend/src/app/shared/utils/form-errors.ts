import { AbstractControl } from '@angular/forms';

/**
 * Renvoie le message d'erreur d'un champ de formulaire, ou null s'il est valide.
 */
export function getFieldErrorMessage(control: AbstractControl): string | null {
  if (!control.touched || !control.errors) {
    return null;
  }

  const errors = control.errors;

  if (errors['required']) return 'Ce champ est obligatoire.';
  if (errors['email']) return "Le format de l'email est invalide.";
  if (errors['maxlength']) return `Maximum ${errors['maxlength'].requiredLength} caractères.`;
  if (errors['min']) return `La valeur doit être supérieure ou égale à ${errors['min'].min}.`;
  if (errors['pattern']) return 'Le format est invalide.';

  return 'Valeur invalide.';
}