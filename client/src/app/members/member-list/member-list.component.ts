import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { take } from 'rxjs';
import { UserParams } from 'src/app/_models/UserParams';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  //member$: Observable<Member[]> | undefined;
  registerForm: FormGroup = new FormGroup({});
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [
    {
      value: 'male', 
      display: 'Males'
    },
    {
      value: 'female', 
      display: 'Females'
    }
  ];

  constructor(private memberService: MembersService) {
    console.log('[member-list] values before memberServices gets userParams:');
    console.log(this.userParams);
    this.userParams = this.memberService.getUserParams();
    if(!this.userParams) console.log('[member-list-constructor] user params is null')
    console.log('[member-list] values after memberServices gets userParams:');
    console.log(this.userParams);
  }

  ngOnInit(): void {
    //this.member$ = this.memberService.getMembers();
    console.log('')
    this.loadMembers();
  }

  loadMembers(){
    if (this.userParams) {
      console.log('[member-list] userParams:');
      console.log(this.userParams);
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      });
    }
  }

  resetFilters(){
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers(); 
  }

  pageChanged(event: any){
    if (this.userParams && this.userParams?.pageNumber !== event.page){
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
