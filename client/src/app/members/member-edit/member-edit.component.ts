import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss']
})
export class MemberEditComponent implements OnInit {
  member: Member | undefined;
  user: User | null = null;


  constructor(private accountService: AccountService, private memberService: MembersService) {
    //after we received one user our request is completed so we don't have to unsubscribe
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user=user
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }

loadMember()
 {
  if(!this.user) return;
  console.log(this.user);
  this.memberService.getMember(this.user.UserName).subscribe({
    next: member => this.member = member
  })
 }
}
