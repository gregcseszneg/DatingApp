import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string = 'https://localhost:5001/api/';
  private currentUserSource = new BehaviorSubject<User | null>(null); //it can be user or null
  currentUser$ = this.currentUserSource.asObservable(); //create the observable

  constructor(private http: HttpClient) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl +'account/login', model).pipe(
        map((response: User) => {
          const user = response;
          if(user)
          {
            localStorage.setItem('user', JSON.stringify(user));
            this.setCurrentUser(user);
          }
        })
      );
  }

  register(model:any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user)
        {
          localStorage.setItem('user', JSON.stringify(user));
          this.setCurrentUser(user);
        }
      })
    )
  }

  logout()
  {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  setCurrentUser(user: User)
  {
    this.currentUserSource.next(user);
  }
}
