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

import { HeroDto } from '../models/hero-dto';
import { HeroStatisticsDto } from '../models/hero-statistics-dto';
import { PlayStyleDto } from '../models/play-style-dto';

@Injectable({
  providedIn: 'root',
})
export class HeroesService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiHeroesGet
   */
  static readonly ApiHeroesGetPath = '/api/Heroes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiHeroesGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesGet$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<HeroDto>>> {

    const rb = new RequestBuilder(this.rootUrl, HeroesService.ApiHeroesGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<HeroDto>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiHeroesGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesGet(params?: {
  },
  context?: HttpContext

): Observable<Array<HeroDto>> {

    return this.apiHeroesGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<HeroDto>>) => r.body as Array<HeroDto>)
    );
  }

  /**
   * Path part for operation apiHeroesStatisticsGet
   */
  static readonly ApiHeroesStatisticsGetPath = '/api/Heroes/statistics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiHeroesStatisticsGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesStatisticsGet$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<HeroStatisticsDto>>> {

    const rb = new RequestBuilder(this.rootUrl, HeroesService.ApiHeroesStatisticsGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<HeroStatisticsDto>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiHeroesStatisticsGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesStatisticsGet(params?: {
  },
  context?: HttpContext

): Observable<Array<HeroStatisticsDto>> {

    return this.apiHeroesStatisticsGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<HeroStatisticsDto>>) => r.body as Array<HeroStatisticsDto>)
    );
  }

  /**
   * Path part for operation apiHeroesStatisticsHeroIdGet
   */
  static readonly ApiHeroesStatisticsHeroIdGetPath = '/api/Heroes/statistics/{heroId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiHeroesStatisticsHeroIdGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesStatisticsHeroIdGet$Response(params: {
    heroId: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<HeroStatisticsDto>> {

    const rb = new RequestBuilder(this.rootUrl, HeroesService.ApiHeroesStatisticsHeroIdGetPath, 'get');
    if (params) {
      rb.path('heroId', params.heroId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<HeroStatisticsDto>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiHeroesStatisticsHeroIdGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiHeroesStatisticsHeroIdGet(params: {
    heroId: string;
  },
  context?: HttpContext

): Observable<HeroStatisticsDto> {

    return this.apiHeroesStatisticsHeroIdGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<HeroStatisticsDto>) => r.body as HeroStatisticsDto)
    );
  }

  /**
   * Path part for operation apiHeroesPlaystyleHeroIdPut
   */
  static readonly ApiHeroesPlaystyleHeroIdPutPath = '/api/Heroes/playstyle/{heroId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiHeroesPlaystyleHeroIdPut()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiHeroesPlaystyleHeroIdPut$Response(params: {
    heroId: string;
    body?: PlayStyleDto
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, HeroesService.ApiHeroesPlaystyleHeroIdPutPath, 'put');
    if (params) {
      rb.path('heroId', params.heroId, {});
      rb.body(params.body, 'application/json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiHeroesPlaystyleHeroIdPut$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiHeroesPlaystyleHeroIdPut(params: {
    heroId: string;
    body?: PlayStyleDto
  },
  context?: HttpContext

): Observable<void> {

    return this.apiHeroesPlaystyleHeroIdPut$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
