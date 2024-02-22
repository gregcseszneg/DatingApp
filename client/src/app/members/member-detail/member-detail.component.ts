import { CommonModule } from '@angular/common';
import { Component, NgModule, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem} from 'ng-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MatTabChangeEvent, MatTabGroup, MatTabsModule } from '@angular/material/tabs';


@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss'],
  imports: [CommonModule, GalleryModule, MatTabsModule]
})
export class MemberDetailComponent implements OnInit{
  member: Member | undefined;
  images: GalleryItem[]=[];
  
  constructor(private memberService: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    var username= this.route.snapshot.paramMap.get('username');
    if(!username) {
      return;
    }
    this.memberService.getMember(username).subscribe({
      next: member =>  {
      this.member = member,
         this.getImages()
      }
    })
    
  }

getImages() {
  if(!this.member) return;
  for (const photo of this.member?.photos) {
    this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
  }
}
}