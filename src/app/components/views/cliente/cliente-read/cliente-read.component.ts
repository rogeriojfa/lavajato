import { Component, OnInit } from '@angular/core';
import { ClienteService } from './../cliente.service';
import { Cliente } from './../cliente.model';

@Component({
  selector: 'app-cliente-read',
  templateUrl: './cliente-read.component.html',
  styleUrls: ['./cliente-read.component.css']
})
export class ClienteReadComponent implements OnInit {

  clientes: Cliente[] = []

  displayedColumns: string[] = ['id', 'nome', 'razaosocial', 'telefone', 'contato', 'acoes'];

  constructor(private cliServ: ClienteService) { }

  ngOnInit(): void { 
    this.findAll();
   }
 
  findAll(){
    this.cliServ.findAll().subscribe(resposta => {
      this.clientes = resposta;
    })
  }

}
