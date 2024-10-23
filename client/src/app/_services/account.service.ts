import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private likeService = inject(LikesService);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      const role = JSON.parse(atob(user.token.split('.')[1])).role // atob is outdated
      return Array.isArray(role) ? role : [role]; // return as array even if single value
    }
    return [];
  });

  login(model: any)
  {
    return this.http.post<User>(this.baseUrl + "account/login", model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any)
  {
    return this.http.post<User>(this.baseUrl + "account/register", model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds(); // updates signal on the service
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
