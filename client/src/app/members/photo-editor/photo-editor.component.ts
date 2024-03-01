import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor () {}

  ngOnInit(): void {}
}
