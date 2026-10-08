import { Injectable } from "@angular/core";
import { HttpHandler, HttpInterceptor, HttpRequest, HttpEvent } from "@angular/common/http";
import { Observable } from "rxjs";
import { finalize } from 'rxjs/operators';
import { MyAuthService } from "./my-auth.service";
import { MyPageProgressbarService } from '../../modules/shared/progressbars/my-page-progressbar.service';

@Injectable()
export class MyAuthInterceptorService implements HttpInterceptor {

  constructor(
    private auth: MyAuthService,
    private progressBarService: MyPageProgressbarService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authToken = this.auth.getLoginToken()?.token;

    const requestToSend = authToken
      ? req.clone({
          setHeaders: {
            Authorization: `Bearer ${authToken}`
          }
        })
      : req;

    this.progressBarService.show();
    return next.handle(requestToSend).pipe(
      finalize(() => this.progressBarService.hide())
    );
  }
}
