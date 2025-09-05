/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpContext } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { MatchLogDto } from '../models/match-log-dto';

@Injectable({
  providedIn: 'root',
})
export class MatchesService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiMatchesGet
   */
  static readonly ApiMatchesGetPath = '/api/Matches';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiMatchesGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiMatchesGet$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MatchLogDto>>> {

    const rb = new RequestBuilder(this.rootUrl, MatchesService.ApiMatchesGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MatchLogDto>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiMatchesGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiMatchesGet(params?: {
  },
  context?: HttpContext

): Observable<Array<MatchLogDto>> {

    return this.apiMatchesGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MatchLogDto>>) => r.body as Array<MatchLogDto>)
    );
  }

}
