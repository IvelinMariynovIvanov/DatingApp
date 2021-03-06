import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
        intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
            return next.handle(req).pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        if (error.status === 401) {
                            return throwError(error.statusText);
                        }
                        const appicationError = error.headers.get('Application-Error');
                        if (appicationError) {
                            console.error(appicationError);
                            return throwError(appicationError);
                        }
                        const serverError = error.error;
                        let modalStateError = '';
                        if (serverError && typeof serverError === 'object') {
                            for (const key in serverError) {
                                if (serverError[key]) {
                                    modalStateError += serverError[key] + '\n';
                                }
                            }
                        }
                        return throwError(modalStateError || serverError || 'Server Error');
                    }
                })
            );
        }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
