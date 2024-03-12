import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
        ToastrModule.forRoot(
      {
        positionClass: 'toast-bottom-right'
      }
    ),
    NgxSpinnerModule.forRoot({
      type: 'ball-pulse'
    }),
    BsDatepickerModule.forRoot(),
    ButtonsModule.forRoot()
  ], exports: [
    ToastrModule, 
    NgxSpinnerModule,
    BsDatepickerModule,
    PaginationModule,
    ButtonsModule
  ]
})
export class SharedModule { }
