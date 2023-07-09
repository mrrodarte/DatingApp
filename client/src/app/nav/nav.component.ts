import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { User } from '../_models/user';
import { UserParams } from '../_models/UserParams';
import { take } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  model: any ={};
  user: User | undefined;
  userParams: UserParams | undefined;
  
  constructor(public accountService:AccountService,
              private router: Router) { 
  }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members');
        this.model = {};
      }
    });
  }
  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
