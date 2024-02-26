import { Component, HostListener, OnInit, ViewChild} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MatTabChangeEvent, MatTabGroup, MatTabsModule } from '@angular/material/tabs';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-member-edit',
  standalone: true,
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss'],
  imports: [CommonModule, MatTabsModule, FormsModule]
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined; //helps to reach a component from the DOM
  
  //helps to detect a given event in the browser
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){ 
    if(this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user: User | null = null;


  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) {
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
  this.memberService.getMember(this.user.userName).subscribe({
    next: member => this.member = member
  })
 }

 updateMember() {
  this.memberService.updateMember(this.editForm?.value).subscribe({
    next: _ => {
      this.toastr.success("profile updated successfully")
      this.editForm?.reset(this.member);
    }
  })
 }
}
