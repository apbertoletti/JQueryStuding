import { Component } from '@angular/core';
import { Nav } from './models/nav';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html'
})
export class MenuComponent {
  menus: Nav[] = [
    {
      link: '/home',
      name: 'Home',
      exact: true,
      admin: false
    },
    {
      link: '/cadastro',
      name: 'Cadastro',
      exact: true,
      admin: false
    },
    {
      link: '/sobre',
      name: 'Sobre',
      exact: true,
      admin: false
    },
    {
      link: '/produto',
      name: 'Produtos',
      exact: false,
      admin: false
    },
    {
      link: '/filme',
      name: 'Filmes',
      exact: false,
      admin: false
    },
    {
      link: '/bar',
      name: 'Bar',
      exact: false,
      admin: false
    },
    {
      link: '/admin',
      name: 'Admin',
      exact: true,
      admin: false
    }
  ]
}
