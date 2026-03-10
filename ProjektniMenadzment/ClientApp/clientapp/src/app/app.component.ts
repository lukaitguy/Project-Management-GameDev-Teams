import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TestService } from './core/services/test.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'clientapp';
  message = '';

  constructor(private testService: TestService) { }

  ngOnInit() {
    this.testService.getMessage().subscribe(res => {
      this.message = res.message;
    })
  }


}
