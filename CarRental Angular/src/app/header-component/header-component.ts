import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header-component',
  imports: [RouterLink, RouterLinkActive ],
  templateUrl: './header-component.html',
  styleUrl: './header-component.css',
})
export class HeaderComponent {
	public navigationMap : Map<string, string> = new Map<string, string>
	([ 	
		["Home", "/home"],
		["Cars", "/cars"],
  		["About", "/about"],
		["Contacts", "/contacts"],
		["Feedbacks", "/feedbacks"]
	]);

	
}
