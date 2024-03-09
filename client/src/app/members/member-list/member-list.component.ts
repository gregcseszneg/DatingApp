import { Component, NgModule, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagiantion } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';



@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss']
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];
  pagination: Pagiantion | undefined;
  pageNumber = 1;
  pageSize = 5;

  constructor(private memberService: MembersService)
  {}

  ngOnInit(): void {
    this.loadMembers();
   }

   loadMembers() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
      next: response => {
        if (response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    })
   }

   pageChanged(event: any) {
    if(this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMembers();
    }
    }
}
