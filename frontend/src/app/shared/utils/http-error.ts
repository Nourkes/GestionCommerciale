import { HttpErrorResponse } from '@angular/common/http';

/**
 * Transforme une erreur HTTP en message lisible pour l'utilisateur.
 * Lit le format ProblemDetails renvoyé par le GlobalExceptionHandler de l'API.
 */
export function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    // status 0 = l'API n'a pas répondu du tout (éteinte, mauvaise URL, CORS...)
    if (error.status === 0) {
      return "Impossible de joindre le serveur. Vérifiez que l'API est démarrée.";
    }

    const problem = error.error;

    // Erreurs de validation (400) : { errors: { "Email": ["Le format..."], ... } }
    if (problem?.errors) {
      return Object.values(problem.errors as Record<string, string[]>).flat().join(' ');
    }

    // Erreurs métier (401, 404, 409) : { detail: "Stock insuffisant..." }
    if (problem?.detail) {
      return problem.detail;
    }
  }
  return 'Une erreur inattendue est survenue.';
}