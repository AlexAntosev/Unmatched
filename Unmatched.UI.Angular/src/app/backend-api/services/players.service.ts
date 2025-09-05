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

import { PlayerDto } from '../models/player-dto';
import { PlayerStatisticsDto } from '../models/player-statistics-dto';

@Injectable({
  providedIn: 'root',
})
export class PlayersService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiPlayersGet
   */
  static readonly ApiPlayersGetPath = '/api/Players';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPlayersGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersGet$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<PlayerDto>>> {

    const rb = new RequestBuilder(this.rootUrl, PlayersService.ApiPlayersGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<PlayerDto>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPlayersGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersGet(params?: {
  },
  context?: HttpContext

): Observable<Array<PlayerDto>> {

    return this.apiPlayersGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<PlayerDto>>) => r.body as Array<PlayerDto>)
    );
  }

  /**
   * Path part for operation apiPlayersStatisticsGet
   */
  static readonly ApiPlayersStatisticsGetPath = '/api/Players/statistics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPlayersStatisticsGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersStatisticsGet$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<PlayerStatisticsDto>>> {

    const rb = new RequestBuilder(this.rootUrl, PlayersService.ApiPlayersStatisticsGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<PlayerStatisticsDto>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPlayersStatisticsGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersStatisticsGet(params?: {
  },
  context?: HttpContext

): Observable<Array<PlayerStatisticsDto>> {

    return this.apiPlayersStatisticsGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<PlayerStatisticsDto>>) => r.body as Array<PlayerStatisticsDto>)
    );
  }

  /**
   * Path part for operation apiPlayersStatisticsHeroIdGet
   */
  static readonly ApiPlayersStatisticsHeroIdGetPath = '/api/Players/statistics/{heroId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPlayersStatisticsHeroIdGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersStatisticsHeroIdGet$Response(params: {
    heroId: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<PlayerStatisticsDto>> {

    const rb = new RequestBuilder(this.rootUrl, PlayersService.ApiPlayersStatisticsHeroIdGetPath, 'get');
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
        return r as StrictHttpResponse<PlayerStatisticsDto>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPlayersStatisticsHeroIdGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPlayersStatisticsHeroIdGet(params: {
    heroId: string;
  },
  context?: HttpContext

): Observable<PlayerStatisticsDto> {

    return this.apiPlayersStatisticsHeroIdGet$Response(params,context).pipe(
      map((r: StrictHttpResponse<PlayerStatisticsDto>) => r.body as PlayerStatisticsDto)
    );
  }

}
