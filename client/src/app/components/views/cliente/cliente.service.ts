import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Cliente } from './cliente.model';
import { environment } from './../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {

    constructor(
    private http: HttpClient
  ) { }

    findAll():Observable<Cliente[]> {
    var url = `${environment.baseUrl}/clientes`
      return this.http.get<Cliente[]>(url)

    }
}
